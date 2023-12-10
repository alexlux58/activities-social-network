import { Cropper } from "react-cropper";
import "cropperjs/dist/cropper.css";

interface Props {
  setCropper: (cropper: Cropper) => void;
  imagePreview: string;
}

const PhotoWidgetCropper = ({ imagePreview, setCropper }: Props) => {
  return (
    <Cropper
      src={imagePreview}
      style={{ height: 200, width: "100%" }}
      // Cropper.js options
      initialAspectRatio={1}
      aspectRatio={1}
      preview=".img-preview"
      guides={false}
      viewMode={1}
      background={false}
      autoCropArea={1}
      onInitialized={(cropper) => setCropper(cropper)}
    />
  );
};

export default PhotoWidgetCropper;
