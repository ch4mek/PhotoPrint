import { AbsenceTransactionHistoryColumnWidths } from './types';

export const absenceTransactionHistoryColumns = {
	transactionDate: {
		key: 'transactionDate',
		sortable: true,
	},
	inquiryTemplateName: {
		key: 'inquiryTemplateName',
		sortable: true,
	},
	description: {
		key: 'description',
		sortable: false,
	},
	used: {
		key: 'used',
		sortable: false,
	},
	accrued: {
		key: 'accrued',
		sortable: false,
	},
	remainder: {
		key: 'remainder',
		sortable: false,
	},
	documents: {
		key: 'files',
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
