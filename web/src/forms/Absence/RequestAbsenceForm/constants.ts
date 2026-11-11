import { DocumentFileTypes } from '@/types/file';

import { InitialDataValues } from './types';

export const REQUEST_POPUP_TITLE = 'Request absence';

export const initialData: InitialDataValues = {
	absenceName: null,
	files: [],
	notes: '',
	dateFrom: null,
	dateTo: null,
	hours: '',
};

export const MIN_HOURS_VALUE = 1;
export const MAX_LENGTH_HOURS = 1;
export const MAXIMUM_HOURS_REQUEST = 8;
export const MAXIMUM_REQUEST_DOCUMENT_COUNT = 10;
export const requestDocumentExtensions = Object.values(DocumentFileTypes);

export const fieldsNames = {
	absenceName: 'absenceName',
	hours: 'hours',
	dateTo: 'dateTo',
	dateFrom: 'dateFrom',
	notes: 'notes',
};
