import { Dispatch, SetStateAction, useMemo } from 'react';
import { Column } from 'react-data-grid';
import { useTranslation } from 'react-i18next';

import {
	columnWidths,
	DEFAULT_EDUCATIONS_FILTER_VALUES,
	EDUCATIONS_GRID_COLUMNS,
} from '../constants';
import { EducationsFilterOptions, EducationsGridRow } from '../types';
import { checkForEmptyFilter } from '../utils/checkForEmptyFilter';

import {
	FreeTextModal,
	getColumnWithTooltipFactory,
	getEditColumnFactory,
	headerCellWithFilterRenderer,
	SelectionModal,
} from '@/components/Grid';
import { getColumnWidth } from '@/components/Grid/utils/getColumnWidth';
import {
	areArraysEqualByPrimitiveItems,
	arePrimitiveValuesEqual,
} from '@/helpers/areValuesEqual';
import {
	EducationsGridFilters,
	EducationsGridFilterSetters,
} from '@/types/educations';

export const useEducationGridColumns = ({
	filters,
	setFilters,
	options,
}: {
	filters: EducationsGridFilters;
	setFilters: EducationsGridFilterSetters;
	options: EducationsFilterOptions;
}) => {
	const { t } = useTranslation('profile', { keyPrefix: 'educationTab' });

	return useMemo(() => {
		const { educationLevels, studyForms } = options;
		const clearFilters = () => {
			setFilters?.clear();
		};

		const columns: Array<Column<EducationsGridRow>> = [
			getColumnWithTooltipFactory<EducationsGridRow>({
				...EDUCATIONS_GRID_COLUMNS.educationLevel,
				name: t('educationLevel'),
				renderHeaderCell: filters
					? (props) =>
							headerCellWithFilterRenderer<EducationsGridRow, string[]>({
								...props,
								filter: filters?.educationLevel || '',
								isFilterApplied: filters.educationLevel.length > 0,
								setFilter: setFilters?.educationLevel as Dispatch<
									SetStateAction<string[]>
								>,
								defaultValue: DEFAULT_EDUCATIONS_FILTER_VALUES.educationLevel,
								disableFilterIconActiveStateAsDefault: true,
								renderModal: SelectionModal,
								areValuesEquals: areArraysEqualByPrimitiveItems,
								options: educationLevels,
							})
					: undefined,
				width: getColumnWidth(columnWidths.educationLevel).width,
			}),

			getColumnWithTooltipFactory<EducationsGridRow>({
				...EDUCATIONS_GRID_COLUMNS.educationalInstitution,
				name: t('educationalInstitution'),
				renderHeaderCell: filters
					? (props) =>
							headerCellWithFilterRenderer<EducationsGridRow, string>({
								...props,
								filter: filters?.educationalInstitution || '',
								isFilterApplied: Boolean(filters.educationalInstitution),
								setFilter: setFilters?.educationalInstitution as Dispatch<
									SetStateAction<string>
								>,
								defaultValue:
									DEFAULT_EDUCATIONS_FILTER_VALUES.educationalInstitution,
								renderModal: FreeTextModal,
								areValuesEquals: arePrimitiveValuesEqual,
							})
					: undefined,
				width: getColumnWidth(columnWidths.educationalInstitution).width,
			}),

			getColumnWithTooltipFactory<EducationsGridRow>({
				...EDUCATIONS_GRID_COLUMNS.faculty,
				name: t('faculty'),
				renderHeaderCell: filters
					? (props) =>
							headerCellWithFilterRenderer<EducationsGridRow, string>({
								...props,
								filter: filters?.faculty || '',
								isFilterApplied: Boolean(filters.faculty),
								setFilter: setFilters?.faculty as Dispatch<
									SetStateAction<string>
								>,
								defaultValue: DEFAULT_EDUCATIONS_FILTER_VALUES.faculty,
								renderModal: FreeTextModal,
								areValuesEquals: arePrimitiveValuesEqual,
							})
					: undefined,
				width: getColumnWidth(columnWidths.faculty).width,
			}),

			getColumnWithTooltipFactory<EducationsGridRow>({
				...EDUCATIONS_GRID_COLUMNS.specialization,
				name: t('specialization'),
				renderHeaderCell: filters
					? (props) =>
							headerCellWithFilterRenderer<EducationsGridRow, string>({
								...props,
								filter: filters?.specialization || '',
								isFilterApplied: Boolean(filters.specialization),
								setFilter: setFilters?.specialization as Dispatch<
									SetStateAction<string>
								>,
								defaultValue: DEFAULT_EDUCATIONS_FILTER_VALUES.specialization,
								renderModal: FreeTextModal,
								areValuesEquals: arePrimitiveValuesEqual,
							})
					: undefined,
				width: getColumnWidth(columnWidths.specialization).width,
			}),

			getColumnWithTooltipFactory<EducationsGridRow>({
				...EDUCATIONS_GRID_COLUMNS.studyForm,
				name: t('studyForm'),
				renderHeaderCell: filters
					? (props) =>
							headerCellWithFilterRenderer<EducationsGridRow, string[]>({
								...props,
								filter: filters?.studyForm || '',
								isFilterApplied: filters.studyForm.length > 0,
								setFilter: setFilters?.studyForm as Dispatch<
									SetStateAction<string[]>
								>,
								defaultValue: DEFAULT_EDUCATIONS_FILTER_VALUES.studyForm,
								disableFilterIconActiveStateAsDefault: true,
								options: studyForms,
								renderModal: SelectionModal,
								areValuesEquals: areArraysEqualByPrimitiveItems,
							})
					: undefined,
				width: getColumnWidth(columnWidths.studyForm).width,
			}),

			getColumnWithTooltipFactory<EducationsGridRow>({
				...EDUCATIONS_GRID_COLUMNS.graduationYear,
				name: t('graduationYear'),
				renderHeaderCell: filters
					? (props) =>
							headerCellWithFilterRenderer({
								...props,
								filter: filters?.graduationYear || '',
								isFilterApplied: Boolean(filters.graduationYear),

								setFilter: setFilters?.graduationYear as Dispatch<
									SetStateAction<string>
								>,
								defaultValue: DEFAULT_EDUCATIONS_FILTER_VALUES.graduationYear,
								renderModal: FreeTextModal,
								areValuesEquals: arePrimitiveValuesEqual,
							})
					: undefined,
				width: getColumnWidth(columnWidths.graduationYear).width,
			}),

			getEditColumnFactory<EducationsGridRow>({
				key: '',
				name: '',
				disabled: filters ? checkForEmptyFilter(filters) : true,
				renderCell: () => null,
				clearFilter: clearFilters,
				displayFilterButton: true,
				sortable: false,
				frozen: true,
			}),
		];

		return columns;
	}, [filters, setFilters, options]);
};
