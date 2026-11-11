import { useState, ChangeEvent } from 'react';

import {
	SUCCESS_PROFILE_PHOTO_CHANGED,
	SUCCESS_PROFILE_PHOTO_DELETE,
	ERROR_PROFILE_PHOTO_MESSAGE,
} from '../constants';
import { ProfileAvatarValues } from '../types';

import { DEFAULT_MAXIMUM_ALLOWED_FILE_SIZE_KB } from '@/constants/allowedFileSizes';
import { imageValidExtensions } from '@/constants/fileExtensions';
import { NotificationType } from '@/constants/notifications';
import { useToast } from '@/context/Toast';
import { createToast } from '@/helpers/createToast';
import { useAttachDocument } from '@/hooks/useAttachDocument';
import { useUploadFile } from '@/queries/useUploadFile';
import { FilePurpose } from '@/types/file';

export const useProfileAvatar = (
	initialValues: ProfileAvatarValues,
	onSubmit: (params: ProfileAvatarValues) => void,
	handleCloseDeleteModal: VoidFunction
) => {
	const toast = useToast();
	const [fileId, setFileId] = useState<string | null>(
		initialValues.avatarFileId
	);
	const { mutateAsync, isPending } = useUploadFile();

	const { handleFileChange } = useAttachDocument(
		imageValidExtensions,
		DEFAULT_MAXIMUM_ALLOWED_FILE_SIZE_KB
	);

	const handleSubmit = (uploadFileId: string | null, message?: string) => {
		const request: ProfileAvatarValues = {
			...initialValues,
			avatarFileId: uploadFileId,
		};

		onSubmit({ ...request, message });
	};
	const setPhotoFile = (photoFile: File) => {
		mutateAsync(
			{ file: photoFile, purpose: FilePurpose.UserAvatar },
			{
				onSuccess: (res) => {
					setFileId(String(res?.data?.fileId));
					handleSubmit(
						String(res?.data?.fileId),
						SUCCESS_PROFILE_PHOTO_CHANGED
					);
				},
				onError: () =>
					toast.open(
						createToast(NotificationType.ERROR, ERROR_PROFILE_PHOTO_MESSAGE)
					),
			}
		);
	};

	const handleImageChange = (event: ChangeEvent<HTMLInputElement>) => {
		const selectedFile = handleFileChange(event);

		if (selectedFile?.length) {
			setPhotoFile(selectedFile[0]);
		}
	};

	const handleDeleteClick = () => {
		setFileId(null);
		handleSubmit(null, SUCCESS_PROFILE_PHOTO_DELETE);
		handleCloseDeleteModal();
	};

	return {
		fileId,
		handleImageChange,
		handleDeleteClick,
		isUploading: isPending,
	};
};
