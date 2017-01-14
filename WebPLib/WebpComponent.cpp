#include "pch.h"
#include "WebpComponent.h"
#include "../webp/decode.h"
#include "../webp/demux.h"
#include <wrl/implements.h>
#include <wrl\client.h>
#include <windows.storage.streams.h>
#include "Robuffer.h" 
using namespace WebpLib;
using namespace Platform;
using namespace Windows::Storage::Streams;
using Windows::Devices::Enumeration::DeviceClass;
using Windows::Devices::Enumeration::DeviceInformation;
using Windows::Devices::Enumeration::DeviceInformationCollection;
using namespace Microsoft::WRL;
using namespace ABI::Windows::Foundation;
using namespace std;

WebpComponent::WebpComponent()
{
}

void WebpComponent::GetInfo(const Array<byte> ^data, __RPC__deref_out_opt int* width, __RPC__deref_out_opt int* height) {
    WebPGetInfo(data->Data, data->Length, width, height);
}


Array<byte> ^ WebpComponent::Parse(const Array<byte> ^data, __RPC__deref_out_opt int* width, __RPC__deref_out_opt int* height) {
    if (!WebPGetInfo(data->Data, data->Length, width, height)) {
        return nullptr;
    }
    byte* pixels = WebPDecodeBGRA(data->Data, data->Length, width, height);
    Array<byte>^ pixelsArray = ref new Array<byte>(pixels, *width * *height * 4);
    delete pixels;
    return pixelsArray;
}

WriteableBitmap^ WebpComponent::Decode(const Array<byte> ^data) {
    int width = 0;
    int height = 0;
    if (!WebPGetInfo(data->Data, data->Length, &width, &height)) {
        return nullptr;
    }

    byte* pixels = WebPDecodeBGRA(data->Data, data->Length, &width, &height);
    WriteableBitmap^ bitmap = ref new WriteableBitmap(width, height);
    IBuffer^ buffer = bitmap->PixelBuffer;
    ComPtr<IBufferByteAccess> pBufferByteAccess;
    ComPtr<IUnknown> pBuffer((IUnknown*)buffer);
    pBuffer.As(&pBufferByteAccess);
    byte *sourcePixels = nullptr;
    pBufferByteAccess->Buffer(&sourcePixels);
    memcpy(sourcePixels, (void *)pixels, width * height * 4);
    bitmap->Invalidate();
    delete pixels;
    return bitmap;
}
