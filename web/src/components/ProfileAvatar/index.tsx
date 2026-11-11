import { Hint } from '@mms/mms-ui-library/components/Hint';
import { PickerButton } from '@mms/mms-ui-library/components/PickerButton';
import {
	ImportIcon,
	BasketIcon,
	EditIcon,
} from '@mms/mms-ui-library/icons/base';
import { MouseEvent, useCallback, useRef, useState, useMemo } from 'react';

import { DEFAULT_MAXIMUM_ALLOWED_FILE_SIZE_KB } from '@/constants/allowedFileSizes';
import { imageValidExtensions } from '@/constants/fileExtensions';
import { useAuth } from '@/context/index';
import { convertKBToMB } from '@/helpers/convertFileSize';
import {
	formatAllowedFileTypesToAccept,
	formatAllowedFileTypesToShow,
} from '@/helpers/formatAllowedFileTypes';
import { getFullName } from '@/helpers/utils';
import { useAddScrollListener } from '@/hooks/useAddScrollListener';
import { useAvatar } from '@/hooks/useAvatar';
import { useResize } from '@/hooks/useResize';
import { useShowModal } from '@/pages/hooks/useShowModal';
import { useDownloadUserAvatar } from '@/queries/files/useDownloadUserAvatar';

import { DeleteProfileAvatar } from './components/DeleteProfileAvatar';
import { ProfileNotification } from './components/ProfileNotification';
import {
	DELETE_TEXT,
	DOWNLOAD_TEXT,
	EDIT_TEXT,
	PROFILE_MOBILE_XS,
} from './constants';
import { useProfileAvatar } from './hooks/useProfileAvatar';
import { ProfileAvatarSquare, ProfileAvatarWrapper } from './styles';
import { ProfileAvatarProps } from './types';

export function ProfileAvatar({ onSubmit, initialValues }: ProfileAvatarProps) {
	const profileAvatarWrapperRef = useRef<HTMLDivElement>(null);
	const hiddenFileInput = useRef<HTMLInputElement>(null);
	const [isAvatarHovered, setIsAvatarHovered] = useState(false);
	const { avatarFileId, lastName, firstName } = useAuth();
	const { handleOpen, isOpen, handleClose } = useShowModal();

	const pickerButtonRef = useRef<HTMLButtonElement>(null);
	const [isOpenPopover, setIsOpenPopover] =
		useAddScrollListener(pickerButtonRef);

	const { fileId, handleImageChange, handleDeleteClick, isUploading } =
		useProfileAvatar(initialValues, onSubmit, handleClose);

	const { mutate: downloadAvatar, isPending: isDownloading } =
		useDownloadUserAvatar();

	const { imgSource } = useAvatar(fileId ?? avatarFileId);

	const [innerWidth] = useResize();

	const hintPosition = useMemo(() => {
		if (innerWidth <= PROFILE_MOBILE_XS) {
			return 'top-start';
		}

		return 'right';
	}, [innerWidth]);

	const handleEditClick = useCallback(() => {
		hiddenFileInput.current?.click();
	}, []);

	const handleDownloadClick = useCallback(() => {
		if (fileId) {
			downloadAvatar({ fileId, fileTitle: getFullName(firstName, lastName) });
		}
	}, [fileId, downloadAvatar, firstName, lastName]);

	const options = useMemo(() => {
		const deleteOption = {
			value: DELETE_TEXT,
			addon: <BasketIcon />,
			onClick: handleOpen,
		};

		const downloadOption = {
			value: DOWNLOAD_TEXT,
			addon: <ImportIcon />,
			onClick: handleDownloadClick,
		};

		return [
			{
				value: EDIT_TEXT,
				addon: <EditIcon />,
				onClick: handleEditClick,
			},
			...(fileId ? [downloadOption, deleteOption] : []),
		];
	}, [fileId]);

	const handleAvatarHover = ({ type }: MouseEvent) => {
		setIsAvatarHovered(type === 'mouseenter');
	};

	const isButtonDisabled = isUploading || isDownloading;

	return (
		<ProfileAvatarWrapper ref={profileAvatarWrapperRef}>
			<PickerButton
				icon={
					<ProfileAvatarSquare
						firstName={firstName}
						lastName={lastName}
						src={imgSource}
						onMouseEnter={handleAvatarHover}
						onMouseLeave={handleAvatarHover}
					/>
				}
				disabled={isButtonDisabled}
				popoverPosition="bottom-start"
				ref={pickerButtonRef}
				{...{ isOpenPopover, setIsOpenPopover, options }}
			/>

			<input
				type="file"
				hidden
				ref={hiddenFileInput}
				onChange={handleImageChange}
				accept={formatAllowedFileTypesToAccept(imageValidExtensions)}
			/>
			<Hint
				open={isAvatarHovered}
				anchorElement={profileAvatarWrapperRef?.current}
				position={hintPosition}
				text={
					<ProfileNotification
						allowedFormats={formatAllowedFileTypesToShow(imageValidExtensions)}
						maximumFileSizeInMb={convertKBToMB(
							DEFAULT_MAXIMUM_ALLOWED_FILE_SIZE_KB
						)}
					/>
				}
			/>
			{isOpen && (
				<DeleteProfileAvatar
					handleClose={handleClose}
					handleSubmit={handleDeleteClick}
				/>
			)}
		</ProfileAvatarWrapper>
	);
}
