import { getFileType } from '@/helpers/getFileType';

export const formatConversationFileType = (fileName: string) => {
	const fileType = getFileType(fileName);

	if (fileType) {
		return `.${fileType}`;
	}

	const extension = fileName.includes('.')
		? fileName.split('.').pop()?.toLowerCase()
		: undefined;

	return extension ? `.${extension}` : '';
};
