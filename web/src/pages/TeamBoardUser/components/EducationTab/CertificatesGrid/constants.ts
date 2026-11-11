import { fromToDateObject } from '@/constants/defaultValues';
import { CertificateFilters } from '@/types/certificate';
import { GridColumnWidth } from '@/types/grid';

import {
	UserCertificatesGridColumnsNames,
	UserCertificatesGridConstants,
} from './types';

export const VIEW_LINK_COLUMN_HEADER_CLASS_NAME = 'view-link-column-header';
export const VIEW_LINK_COLUMN_CELL_CLASS_NAME = 'view-link-column-cell';
export const CERTIFICATE_IMAGE_SIZE = 100;

const minColumnWidths = {
	xs: 37,
	s: 100,
	m: 150,
	l: 350,
};

export const defaultUserCertificatesFilters: CertificateFilters = {
	certificateId: '',
	stack: '',
	receiptDate: fromToDateObject,
};

export const certificatesGridColumns: UserCertificatesGridConstants<UserCertificatesGridColumnsNames> =
	{
		technology: {
			key: 'technology',
			name: 'Specialization',
		},
		certificateId: {
			key: 'certificateId',
			name: 'Certificate id',
		},
		stack: {
			key: 'stack',
			name: 'Stack',
		},
		receiptDate: {
			key: 'receiptDate',
			name: 'Date',
		},
		certificateViewLink: {
			key: 'certificateViewLink',
			name: '',
		},
	};

export const certificatesGridColumnWidths: UserCertificatesGridConstants<GridColumnWidth> =
	{
		technology: minColumnWidths.l,
		certificateId: minColumnWidths.m,
		stack: minColumnWidths.m,
		receiptDate: minColumnWidths.s,
		certificateViewLink: {
			width: minColumnWidths.xs,
			fixed: true,
		},
	};

export const RESET_FILTER_COLUMN_INDEX =
	Object.values(certificatesGridColumns).length + 1;
