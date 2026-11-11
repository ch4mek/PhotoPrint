import { PdfIcon } from '@mms/mms-ui-library/icons/fileTypes/PdfIcon';
import { PngIcon } from '@mms/mms-ui-library/icons/fileTypes/PngIcon';
import { PptIcon } from '@mms/mms-ui-library/icons/fileTypes/PptIcon';
import { TxtIcon } from '@mms/mms-ui-library/icons/fileTypes/TxtIcon';
import { WordIcon } from '@mms/mms-ui-library/icons/fileTypes/WordIcon';
import { XlsxIcon } from '@mms/mms-ui-library/icons/fileTypes/XlsxIcon';
import { ZipIcon } from '@mms/mms-ui-library/icons/fileTypes/ZipIcon';
import { ElementType } from 'react';

import { DocumentFileTypes } from '@/types/file';

export const fileExtensions = {
	xlsx: 'xlsx',
	excel: 'excel',
	excelAccount: 'xlsx_account',
	pdf: 'pdf',
	word: 'word',
};

export const fileExtensionMapping = {
	xlsx: 'excel',
	pdf: 'pdf',
	word: 'word',
};

export const imageValidExtensions = ['jpg', 'jpeg', 'png'];

export const attachedAllFileExtension = Object.values(DocumentFileTypes);

export const FileTypeIcons: Record<DocumentFileTypes, ElementType> = {
	[DocumentFileTypes.Docx]: WordIcon,
	[DocumentFileTypes.Doc]: WordIcon,
	[DocumentFileTypes.Xlsx]: XlsxIcon,
	[DocumentFileTypes.Xls]: XlsxIcon,
	[DocumentFileTypes.Pdf]: PdfIcon,
	[DocumentFileTypes.Jpg]: PngIcon,
	[DocumentFileTypes.Jpeg]: PngIcon,
	[DocumentFileTypes.Png]: PngIcon,
	[DocumentFileTypes.Txt]: TxtIcon,
	[DocumentFileTypes.Pptx]: PptIcon,
	[DocumentFileTypes.Zip]: ZipIcon,
	[DocumentFileTypes.Md]: TxtIcon,
	[DocumentFileTypes.Rar]: ZipIcon,
};
