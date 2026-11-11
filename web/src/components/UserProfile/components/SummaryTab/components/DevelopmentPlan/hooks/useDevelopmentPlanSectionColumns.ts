import { useMemo } from 'react';
import { Column } from 'react-data-grid';
import { useTranslation } from 'react-i18next';

import { columnsWidth, developmentPlanColumnsKeys } from '../constants';
import { DevelopmentPlanSectionRow } from '../types';

import {
	getColumnWithTooltipFactory,
	getEditColumnFactory,
} from '@/components/Grid';
import { DynamicHeightCellRenderer } from '@/components/Grid/renderers/cell/dynamicHeightCellRenderer';
import { tagCellRenderer } from '@/components/Grid/renderers/cell/tagCellRenderer';
import {
	DEVELOPMENT_PLAN_STATUS_CELL_CLASS_NAME,
	DEVELOPMENT_PLAN_STATUS_TAG_WIDTH,
} from '@/constants/developmentPlan';
import { getDevelopmentStatusTagColor } from '@/helpers/getDevelopmentStatusTagColor';
import { SECTION_TEXT_MAX_ROW_COUNT } from '@/pages/TeamBoardUser/constants';
import { DevelopmentPlanStatus } from '@/types/developmentPlan';

export const useDevelopmentPlanSectionColumns =
	(): Column<DevelopmentPlanSectionRow>[] => {
		const { t } = useTranslation('profile', { keyPrefix: 'developmentPlan' });

		return useMemo(
			() => [
				getColumnWithTooltipFactory({
					key: developmentPlanColumnsKeys.status,
					name: t('status'),
					width: columnsWidth.status,
					cellClass: DEVELOPMENT_PLAN_STATUS_CELL_CLASS_NAME,

					renderCell: (props) =>
						props.row.status !== DevelopmentPlanStatus.None
							? tagCellRenderer(
									props,
									getDevelopmentStatusTagColor,
									undefined,
									DEVELOPMENT_PLAN_STATUS_TAG_WIDTH
							  )
							: undefined,
				}),
				getColumnWithTooltipFactory({
					key: developmentPlanColumnsKeys.deadlineDate,
					name: t('deadlineDate'),
					width: columnsWidth.deadlineDate,
				}),
				getColumnWithTooltipFactory({
					key: developmentPlanColumnsKeys.summary,
					name: t('summary'),
					width: columnsWidth.summary,
					renderCell: (props) =>
						DynamicHeightCellRenderer(props, SECTION_TEXT_MAX_ROW_COUNT),
				}),
				getEditColumnFactory({
					key: '',
					name: '',
					frozen: true,
					disabled: true,
					displayFilterButton: false,
					sortable: false,
				}),
			],
			[]
		);
	};
