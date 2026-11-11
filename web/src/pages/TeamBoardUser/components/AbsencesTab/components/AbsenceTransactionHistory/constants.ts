import { AbsenceTransactionHistoryColumnWidths } from './types';

export const absenceTransactionHistoryColumns = {
	transactionDate: {
		key: 'transactionDate',
		name: 'Date',
		sortable: true,
	},
	inquiryTemplateName: {
		key: 'inquiryTemplateName',
		name: 'Request type',
		sortable: true,
	},
	description: {
		key: 'description',
		name: 'Description',
		sortable: false,
	},
	used: {
		key: 'used',
		name: 'Used',
		sortable: false,
	},
	accrued: {
		key: 'accrued',
		name: 'Accrued',
		sortable: false,
	},
	remainder: {
		key: 'remainder',
		name: 'Remainder (Balance)',
		sortable: false,
	},
	documents: {
		key: 'files',
		name: 'Documents',
		sortable: false,
	},
};

export const absenceTransactionHistoryWidths: AbsenceTransactionHistoryColumnWidths =
	{
		transactionDate: {
			width: 100,
			maxWidth: 200,
		},
		inquiryTemplateName: 200,
		description: 200,
		used: {
			width: 61,
			maxWidth: 200,
		},
		accrued: {
			width: 82,
			maxWidth: 200,
		},
		remainder: {
			width: 130,
			maxWidth: 200,
		},
		documents: 180,
	};

export const MIN_ROWS_COUNT = 3;
