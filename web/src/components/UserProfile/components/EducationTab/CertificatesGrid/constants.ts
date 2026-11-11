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
		},
		certificateId: {
			key: 'certificateId',
		},
		stack: {
			key: 'stack',
		},
		receiptDate: {
			key: 'receiptDate',
		},
		certificateViewLink: {
			key: 'certificateViewLink',
		},
	};

export const certificatesGridColumnWidths: UserCertificatesGridConstants<GridColumnWidth> =
	{
		technology: `minmax(${minColumnWidths.l}px, 1fr)`,
		certificateId: `minmax(${minColumnWidths.m}px, 1fr)`,
		stack: `minmax(${minColumnWidths.m}px, 1fr)`,
		receiptDate: `minmax(${minColumnWidths.s}px, 1fr)`,
		certificateViewLink: {
			width: minColumnWidths.s,
			fixed: true,
		},
	};

export const RESET_FILTER_COLUMN_INDEX =
	Object.values(certificatesGridColumns).length + 1;
