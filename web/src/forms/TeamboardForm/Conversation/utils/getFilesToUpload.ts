import { ConversationFormDocument } from '@/types/document';

export const getFilesToUpload = (
	fileGroups: ConversationFormDocument[]
): File[] =>
	fileGroups
		.map((uploadedFile) => uploadedFile.file)
		.filter((file): file is File => file instanceof File);
