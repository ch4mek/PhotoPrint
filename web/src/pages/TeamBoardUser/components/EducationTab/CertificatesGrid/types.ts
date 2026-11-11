import { GridPaginationProps } from '@mms/mms-ui-library/components/Grids';
import { Dispatch, SetStateAction } from 'react';
import { Column, SortColumn } from 'react-data-grid';

import { GridCellDefaultProps } from '@/components/Grid';
import { CertificateFilters } from '@/types/certificate';

export interface UserCertificatesRow extends GridCellDefaultProps {
	certificateImageURL?: string;
	technology: string;
	certificateId: number;
	userId: number;
	stack: string;
	receiptDate: string;
	certificateViewLink: string;
}

export type UserCertificatesGridConstants<T> = {
	[key in keyof UserCertificatesRow]: T;
};

export interface UserCertificatesGridColumnsNames {
	key: keyof UserCertificatesRow;
	name: string;
}

export enum UserCertificatesFilterActions {
	RESET = 'reset',
	CERTIFICATE_ID = 'certificateId',
	STACK = 'stack',
	RECEIPT_DATE = 'receiptDate',
}

export type UserCertificatesReducerAction = {
	[K in UserCertificatesFilterActions]: {
		type: K;
		payload: K extends keyof CertificateFilters
			? CertificateFilters[K]
			: CertificateFilters;
	};
}[UserCertificatesFilterActions];

export type UserCertificatesFiltersHandlers = {
	[K in UserCertificatesFilterActions]: K extends keyof CertificateFilters
		? Dispatch<SetStateAction<CertificateFilters[K]>>
		: VoidFunction;
};

export type UserCertificatesFiltersReducer = [
	CertificateFilters,
	UserCertificatesFiltersHandlers
];

export type UserCertificatesGridProps = {
	rows: Array<UserCertificatesRow>;
	columns: Array<Column<UserCertificatesRow>>;
	pagination?: GridPaginationProps;
	handleSortColumnChange: (newSortColumns: Array<SortColumn>) => void;
	sortColumns: Array<SortColumn>;
	handleCertificateFormToggle: VoidFunction;
};
