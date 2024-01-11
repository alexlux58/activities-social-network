import { observer } from "mobx-react-lite";
import { Button, Card, Header, Tab, Image, Grid } from "semantic-ui-react";
import { IPhoto, Profile } from "../../app/models/profile";
import { useStore } from "../../app/stores/store";
import { SyntheticEvent, useState } from "react";
import PhotoUploadWidget from "../../app/common/imageUpload/PhotoUploadWidget";

interface Props {
  profile: Profile;
}

const ProfilePhotos = ({ profile }: Props) => {
  const [addPhotoMode, setAddPhotoMode] = useState(false);
  const [target, setTarget] = useState("");

  const { profileStore } = useStore();

  if (!profileStore) return null;

  const {
    isCurrentUser,
    uploadPhoto,
    uploading,
    loadingProfile,
    setMainPhoto,
    deletePhoto,
  } = profileStore;

  function handlePhotoUpload(file: Blob) {
    uploadPhoto(file).then(() => setAddPhotoMode(false));
  }

  function hadnleSetMainPhoto(
    photo: IPhoto,
    e: SyntheticEvent<HTMLButtonElement>
  ) {
    setTarget(e.currentTarget.name);
    setMainPhoto(photo);
  }

  function handleDeletePhoto(
    photo: IPhoto,
    e: SyntheticEvent<HTMLButtonElement>
  ) {
    setTarget(e.currentTarget.name);
    deletePhoto(photo);
  }

  return (
    <Tab.Pane>
      <Grid>
        <Grid.Column width={16} style={{ paddingBottom: 0 }}>
          {/* <Header floated="left" icon="user" content={`Photos`} /> */}
          <Header icon="image" content="Photos" />
          {isCurrentUser && (
            <Button
              floated="right"
              basic
              content={addPhotoMode ? "Cancel" : "Add Photo"}
              onClick={() => setAddPhotoMode(!addPhotoMode)}
            />
          )}
        </Grid.Column>
        <Grid.Column width={16}>
          {addPhotoMode ? (
            <PhotoUploadWidget
              uploadPhoto={handlePhotoUpload}
              loading={uploading}
            />
          ) : (
            <Card.Group itemsPerRow={5}>
              {profile.photos?.map((photo) => (
                <Card key={photo.id}>
                  <Image src={photo.url} />
                  {isCurrentUser && (
                    <Button.Group fluid widths={2}>
                      <Button
                        basic
                        positive
                        content="Main"
                        name={"main" + photo.id}
                        onClick={(e) => hadnleSetMainPhoto(photo, e)}
                        disabled={photo.isMain}
                        loading={target === "main" + photo.id && loadingProfile}
                      />
                      <Button
                        basic
                        negative
                        icon="trash"
                        name={photo.id}
                        disabled={photo.isMain}
                        onClick={(e) => handleDeletePhoto(photo, e)}
                        loading={target === photo.id && loadingProfile}
                      />
                    </Button.Group>
                  )}
                </Card>
              ))}
            </Card.Group>
          )}
        </Grid.Column>
      </Grid>
    </Tab.Pane>
  );
};

export default observer(ProfilePhotos);
