import React, { Dispatch, SetStateAction, useMemo } from 'react';
import { Column } from 'react-data-grid';

import { DatesRenderCell } from '../components/datesRenderCell';
import { PerformanceReviewGridRow, UseGetGridColumnsProps } from '../types';

import {
	FreeTextAutoComplete,
	FreeTextModal,
	getColumnWithTooltipFactory,
	getEditColumnFactory,
	headerCellRenderer,
	headerCellWithFilterRenderer,
	SelectionModal,
} from '@/components/Grid';
import { cellWithIconRenderer } from '@/components/Grid/renderers/cell/cellWithIconRenderer';
import { getColumnWidth } from '@/components/Grid/utils/getColumnWidth';
import {
	areArraysEqualByPrimitiveItems,
	arePrimitiveValuesEqual,
} from '@/helpers/areValuesEqual';
import { PERFORMANCE_GRID_COLUMNS } from '@/pages/PerformanceReview/components/PerformanceReviewGrid/constants';
import { isResetDisabled } from '@/pages/PerformanceReview/components/PerformanceReviewGrid/utils/isResetDisabled';

export function useGetGridColumns({
	filters,
	filterSetter,
	options,
	gridColumnsWidths,
	infoDateTagContent,
	handleTagChoose,
	handleIconClick,
	handleContentClick,
	enableCellClick,
}: UseGetGridColumnsProps) {
	const { users, departments } = options;

	const columns: Array<Column<PerformanceReviewGridRow>> = useMemo(
		() => [
			getColumnWithTooltipFactory({
				...PERFORMANCE_GRID_COLUMNS.performanceId,
				width: getColumnWidth(gridColumnsWidths.id).width,
				renderHeaderCell: (props) =>
					headerCellWithFilterRenderer({
						...props,
						filter: filters.id,
						isFilterApplied: !!filters.id,
						setFilter: filterSetter.id as Dispatch<SetStateAction<string>>,
						defaultValue: '',
						renderModal: FreeTextModal,
						areValuesEquals: arePrimitiveValuesEqual,
					}),
			}),

			getColumnWithTooltipFactory({
				...PERFORMANCE_GRID_COLUMNS.performanceFullName,
				width: getColumnWidth(gridColumnsWidths.fullName).width,
				renderHeaderCell: (props) =>
					headerCellWithFilterRenderer({
						...props,
						filter: filters.fullName,
						isFilterApplied: !!filters.fullName,
						setFilter: filterSetter.fullName as Dispatch<
							SetStateAction<string>
						>,
						defaultValue: '',
						renderModal: FreeTextAutoComplete,
						autocompleteOptions: users,
						areValuesEquals: arePrimitiveValuesEqual,
					}),
				renderCell: (props) =>
					cellWithIconRenderer(props, {
						onIconClick: handleIconClick,
						onContentClick: enableCellClick(props.row)
							? handleContentClick
							: undefined,
					}),
			}),

			getColumnWithTooltipFactory({
				...PERFORMANCE_GRID_COLUMNS.performanceUserDepartment,
				width: getColumnWidth(gridColumnsWidths.departments).width,
				renderHeaderCell: (props) =>
					headerCellWithFilterRenderer({
						...props,
						filter: filters.departments || [],
						setFilter: filterSetter.departments as unknown as React.Dispatch<
							React.SetStateAction<string[]>
						>,
						defaultValue: [],
						renderModal: SelectionModal,
						options: departments,
						isFilterApplied: !!filters.departments?.length,
						areValuesEquals: areArraysEqualByPrimitiveItems,
						disableFilterIconActiveStateAsDefault: true,
					}),
			}),

			getColumnWithTooltipFactory({
				...PERFORMANCE_GRID_COLUMNS.performanceDates,
				renderHeaderCell: (props) =>
					headerCellRenderer({ ...props, tooltipContent: infoDateTagContent }),
				width: getColumnWidth(gridColumnsWidths.dates).width,
				renderCell: (props) => DatesRenderCell(props, handleTagChoose),
			}),

			getEditColumnFactory({
				...PERFORMANCE_GRID_COLUMNS.performanceFilter,
				disabled: isResetDisabled(filters),
				clearFilter: filterSetter.reset,
				renderCell: null,
			}),
		],
		[users, departments, filters, gridColumnsWidths]
	);

	return useMemo(() => columns.filter(({ width }) => width !== 0), [columns]);
}
