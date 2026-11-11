import { ImportIcon } from '@mms/mms-ui-library/icons/base';

import { getFullName } from '@/helpers/utils';
import { useDownloadUserAvatar } from '@/queries/files/useDownloadUserAvatar';

import { ICON_SIZE } from './constants';
import { AvatarContainer, AvatarImage, AvatarOverlay } from './styles';
import { UserAvatarProps } from './types';

export const UserAvatar = ({
	src,
	avatarFileId,
	firstName,
	lastName,
}: UserAvatarProps) => {
	const { mutate: downloadAvatar, isPending } = useDownloadUserAvatar();

	const handleDownload = () => {
		if (!avatarFileId || isPending) {
			return;
		}

		downloadAvatar({
			fileId: avatarFileId,
			fileTitle: getFullName(firstName, lastName),
		});
	};

	return (
		<AvatarContainer>
			<AvatarImage {...{ src, firstName, lastName }} />

			{avatarFileId && (
				<AvatarOverlay onClick={handleDownload}>
					<ImportIcon width={ICON_SIZE} height={ICON_SIZE} />
				</AvatarOverlay>
			)}
		</AvatarContainer>
	);
};
