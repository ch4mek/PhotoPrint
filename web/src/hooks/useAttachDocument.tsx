import {
	MAXIMUM_ALLOWED_FILE_SIZE,
	MAXIMUM_NUMBER_OF_FILES,
	MINIMUM_ALLOWED_FILE_SIZE,
	NotificationType,
	RIGHT_EXTENSIONS,
} from '@/constants/notifications';
import { useToast } from '@/context/Toast';
import { convertKBToMB } from '@/helpers/convertFileSize';
import { createToast } from '@/helpers/createToast';
import { formatAllowedFileTypesToShow } from '@/helpers/formatAllowedFileTypes';
import { useFileSelect } from '@/hooks/useFileSelect';

export const useAttachDocument = (
	validExtensions: string[],
	maxFileSizeInKB?: number,
	minFileSizeInKB?: number
) => {
	const toast = useToast();

	const showErrorToast = (toastMessage: string) => {
		toast.open(createToast(NotificationType.ERROR, toastMessage));
	};

	const onInvalidExtensionError = () => {
		showErrorToast(
			RIGHT_EXTENSIONS.message(formatAllowedFileTypesToShow(validExtensions))
		);
	};

	const onExceededSizeError = (validMaxFileSizeInKB: number) => {
		const validMaxFileSizeInMB = convertKBToMB(validMaxFileSizeInKB);

		showErrorToast(MAXIMUM_ALLOWED_FILE_SIZE.message(validMaxFileSizeInMB));
	};

	const onInsufficientSizeError = (validMinFileSizeInKB: number) => {
		const validMinFileSizeInMB = convertKBToMB(validMinFileSizeInKB);

		showErrorToast(MINIMUM_ALLOWED_FILE_SIZE.message(validMinFileSizeInMB));
	};

	const onExceededCountError = (
		maxFilesCount: number,
		attachedFiles: number
	) => {
		if (attachedFiles > 0) {
			showErrorToast(
				MAXIMUM_NUMBER_OF_FILES.message(maxFilesCount, attachedFiles)
			);
		}
	};

	const { files, removeFile, handleFileChange, handleUploadRawFiles } =
		useFileSelect({
			validExtensions,
			maxFileSizeInKB,
			minFileSizeInKB,
			onExceededSizeError,
			onInsufficientSizeError,
			onInvalidExtensionError,
			onExceededCountError,
		});

	return {
		files,
		removeFile,
		handleFileChange,
		handleUploadRawFiles,
		onExceededSizeError,
		onInvalidExtensionError,
		onExceededCountError,
	};
};
