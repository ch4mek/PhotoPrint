import { getFileNameWithExtension } from '@/helpers/getFileNameWithExtension';
import {
	ConversationFormDocument,
	CreateConversationFileDto,
} from '@/types/document';

export const prepareConversationFilesForRequest = (
	files: ConversationFormDocument[]
): CreateConversationFileDto[] =>
	files.map(({ fileId, fileName, fileSize, fileType }) => ({
		fileId: String(fileId),
		fileName: fileType
			? getFileNameWithExtension(fileName, fileType)
			: fileName,
		fileSize,
	}));
