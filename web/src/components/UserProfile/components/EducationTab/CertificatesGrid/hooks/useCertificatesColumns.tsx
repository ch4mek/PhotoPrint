import { useMemo } from 'react';
import { Column } from 'react-data-grid';
import { useTranslation } from 'react-i18next';

import { SpecializationCell } from '../components/SpecializationCell';
import { ViewCertificateCell } from '../components/ViewCertificateCell';
import {
	certificatesGridColumns,
	certificatesGridColumnWidths,
	defaultUserCertificatesFilters,
	VIEW_LINK_COLUMN_CELL_CLASS_NAME,
	VIEW_LINK_COLUMN_HEADER_CLASS_NAME,
} from '../constants';
import { UserCertificatesFiltersHandlers, UserCertificatesRow } from '../types';
import { isResetDisabled } from '../utils';

import {
	FreeTextModal,
	getColumnWithTooltipFactory,
	getEditColumnFactory,
	headerCellWithFilterRenderer,
} from '@/components/Grid';
import { FromToDateModal } from '@/components/Grid/modals/fromToDateModalRenderer';
import { getColumnWidth } from '@/components/Grid/utils/getColumnWidth';
import {
	areFromToCompatibleObjectsEqual,
	arePrimitiveValuesEqual,
} from '@/helpers/areValuesEqual';
import { CertificateFilters } from '@/types/certificate';

export function useCertificatesColumns(
	filters: CertificateFilters,
	setFilters: UserCertificatesFiltersHandlers
) {
	const { t } = useTranslation('profile', {
		keyPrefix: 'educationTab.certificates',
	});
	const columns: Column<UserCertificatesRow>[] = useMemo(
		() => [
			getColumnWithTooltipFactory({
				...certificatesGridColumns.technology,
				name: t('technology'),
				width: getColumnWidth(certificatesGridColumnWidths.technology).width,
				sortable: true,
				renderCell: SpecializationCell,
			}),
			getColumnWithTooltipFactory({
				...certificatesGridColumns.certificateId,
				name: t('certificateId'),
				width: getColumnWidth(certificatesGridColumnWidths.certificateId).width,
				sortable: true,

				renderHeaderCell: (props) =>
					headerCellWithFilterRenderer({
						...props,
						filter: filters.certificateId,
						setFilter: setFilters.certificateId,
						renderModal: FreeTextModal,
						defaultValue: defaultUserCertificatesFilters.certificateId,
						isFilterApplied: Boolean(filters.certificateId),
						areValuesEquals: arePrimitiveValuesEqual,
					}),
			}),
			getColumnWithTooltipFactory({
				...certificatesGridColumns.stack,
				name: t('stack'),
				width: getColumnWidth(certificatesGridColumnWidths.stack).width,
				sortable: true,

				renderHeaderCell: (props) =>
					headerCellWithFilterRenderer({
						...props,
						filter: filters.stack,
						setFilter: setFilters.stack,
						renderModal: FreeTextModal,
						defaultValue: defaultUserCertificatesFilters.stack,
						isFilterApplied: Boolean(filters.stack),
						areValuesEquals: arePrimitiveValuesEqual,
					}),
			}),
			getColumnWithTooltipFactory({
				...certificatesGridColumns.receiptDate,
				name: t('receiptDate'),
				width: getColumnWidth(certificatesGridColumnWidths.receiptDate).width,
				sortable: true,

				renderHeaderCell: (props) =>
					headerCellWithFilterRenderer({
						...props,
						filter: filters.receiptDate,
						setFilter: setFilters.receiptDate,
						renderModal: FromToDateModal,
						defaultValue: defaultUserCertificatesFilters.receiptDate,
						isFilterApplied:
							!!filters.receiptDate?.from || !!filters.receiptDate?.to,
						areValuesEquals: areFromToCompatibleObjectsEqual,
					}),
			}),
			getEditColumnFactory({
				...certificatesGridColumns.certificateViewLink,
				name: '',
				width: getColumnWidth(certificatesGridColumnWidths.certificateViewLink)
					.width,
				frozen: true,

				headerCellClass: VIEW_LINK_COLUMN_HEADER_CLASS_NAME,
				cellClass: VIEW_LINK_COLUMN_CELL_CLASS_NAME,
				disabled: isResetDisabled(filters),
				clearFilter: setFilters.reset,
				renderCell: ViewCertificateCell,
			}),
		],
		[filters]
	);

	return columns;
}
