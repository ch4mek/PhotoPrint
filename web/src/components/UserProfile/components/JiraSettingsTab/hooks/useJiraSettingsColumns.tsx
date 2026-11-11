import { BasketIcon } from '@mms/mms-ui-library/icons/base/BasketIcon';
import { EditIcon } from '@mms/mms-ui-library/icons/base/EditIcon';
import { Dispatch, SetStateAction, useMemo } from 'react';
import { Column } from 'react-data-grid';

import {
	jiraSettingsGridColumns,
	jiraSettingsMenuOptionsValues,
	JIRA_SETTINGS_PAT_MASK_LENGTH,
} from '../constants';
import { UseJiraSettingsColumnsProps } from '../types';

import {
	cellWithTooltipRenderer,
	FreeTextModal,
	getColumnWithTooltipFactory,
	getEditColumnFactory,
	headerCellRenderer,
	headerCellWithFilterRenderer,
} from '@/components/Grid';
import {
	EDIT_COLUMN_CELL_CLASS,
	EDIT_COLUMN_HEADER_CLASS,
} from '@/components/Grid/constants';
import { HiddenValueCellRenderer } from '@/components/Grid/renderers/cell';
import { menuOptionsCellRenderer } from '@/components/Grid/renderers/cell/menuOptionsCellRenderer';
import { arePrimitiveValuesEqual } from '@/helpers/areValuesEqual';
import { JiraSettingsGridRow } from '@/types/jira';

export const useJiraSettingsColumns = ({
	filters,
	isResetFiltersDisabled,
	setFilters,
	handleDelete,
	handleEdit,
}: UseJiraSettingsColumnsProps) => {
	const columns: Array<Column<JiraSettingsGridRow>> = useMemo(() => {
		const { url, email, projectName, projectTag } = filters;

		return [
			getColumnWithTooltipFactory({
				...jiraSettingsGridColumns.url,
				renderHeaderCell: (props) =>
					headerCellWithFilterRenderer({
						...props,
						filter: url,
						setFilter: setFilters.url as Dispatch<SetStateAction<string>>,
						defaultValue: '',
						renderModal: FreeTextModal,
						isFilterApplied: !!url,
						areValuesEquals: arePrimitiveValuesEqual,
					}),
				renderCell: (props) =>
					cellWithTooltipRenderer(props, {
						isCopyEnabled: true,
					}),
			}),
			getColumnWithTooltipFactory({
				...jiraSettingsGridColumns.email,
				renderHeaderCell: (props) =>
					headerCellWithFilterRenderer({
						...props,
						filter: email,
						setFilter: setFilters.email as Dispatch<SetStateAction<string>>,
						defaultValue: '',
						renderModal: FreeTextModal,
						isFilterApplied: !!email,
						areValuesEquals: arePrimitiveValuesEqual,
					}),
				renderCell: (props) =>
					cellWithTooltipRenderer(props, {
						isCopyEnabled: true,
					}),
			}),
			getColumnWithTooltipFactory({
				...jiraSettingsGridColumns.personalAccessToken,
				renderHeaderCell: (props) =>
					headerCellRenderer({
						...props,
					}),
				renderCell: (props) =>
					HiddenValueCellRenderer(props, {
						options: { maskRepeatCount: JIRA_SETTINGS_PAT_MASK_LENGTH },
					}),
			}),
			getColumnWithTooltipFactory({
				...jiraSettingsGridColumns.projectTag,
				renderHeaderCell: (props) =>
					headerCellWithFilterRenderer({
						...props,
						filter: projectTag,
						setFilter: setFilters.projectTag as Dispatch<
							SetStateAction<string>
						>,
						defaultValue: '',
						renderModal: FreeTextModal,
						isFilterApplied: !!projectTag,
						areValuesEquals: arePrimitiveValuesEqual,
					}),
				renderCell: (props) =>
					cellWithTooltipRenderer(props, {
						isCopyEnabled: true,
					}),
			}),
			getColumnWithTooltipFactory({
				...jiraSettingsGridColumns.projectName,
				renderHeaderCell: (props) =>
					headerCellWithFilterRenderer({
						...props,
						filter: projectName,
						setFilter: setFilters.projectName as Dispatch<
							SetStateAction<string>
						>,
						defaultValue: '',
						renderModal: FreeTextModal,
						isFilterApplied: !!projectName,
						areValuesEquals: arePrimitiveValuesEqual,
					}),
				renderCell: (props) =>
					cellWithTooltipRenderer(props, {
						isCopyEnabled: true,
					}),
			}),

			getEditColumnFactory({
				key: '',
				name: '',
				disabled: isResetFiltersDisabled,
				renderCell: (props) =>
					menuOptionsCellRenderer<JiraSettingsGridRow>(props, {
						options: (row) => [
							{
								value: jiraSettingsMenuOptionsValues.EDIT,
								addon: <EditIcon />,
								onClick: () => handleEdit(row),
							},
							{
								value: jiraSettingsMenuOptionsValues.DELETE,
								addon: <BasketIcon />,
								onClick: () => handleDelete(row),
							},
						],
					}),
				clearFilter: setFilters.reset,
				frozen: true,
				sortable: false,
				headerCellClass: EDIT_COLUMN_HEADER_CLASS,
				cellClass: EDIT_COLUMN_CELL_CLASS,
			}),
		];
	}, [filters, setFilters]);

	return columns;
};
