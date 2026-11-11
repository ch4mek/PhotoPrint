import { PdfIcon } from '@mms/mms-ui-library/icons/fileTypes/PdfIcon';
import { WordIcon } from '@mms/mms-ui-library/icons/fileTypes/WordIcon';
import { XlsxIcon } from '@mms/mms-ui-library/icons/fileTypes/XlsxIcon';
import { ZipIcon } from '@mms/mms-ui-library/icons/fileTypes/ZipIcon';

import { DocumentFileType } from '@/types/notifications';

export const getFileTypeIcon = (fileType: DocumentFileType) => {
	switch (fileType) {
		case DocumentFileType.Excel:
			return XlsxIcon;
		case DocumentFileType.Word:
			return WordIcon;
		case DocumentFileType.Pdf:
			return PdfIcon;
		case DocumentFileType.Archive:
			return ZipIcon;
		default:
			return XlsxIcon;
	}
};
