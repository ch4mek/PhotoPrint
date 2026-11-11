import { DocumentFileTypes } from '@/types/file';

import { InitialDataValues } from './types';

export const REQUEST_DOCUMENTS_POPUP_TITLE = 'Request documents';

export const initialData: InitialDataValues = {
	documentType: null,
	files: [],
	notes: '',
};

export const MAXIMUM_REQUEST_DOCUMENT_COUNT = 10;
export const requestDocumentExtensions = Object.values(DocumentFileTypes);
