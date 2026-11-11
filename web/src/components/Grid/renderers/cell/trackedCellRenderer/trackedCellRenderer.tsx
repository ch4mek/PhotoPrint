import { Tooltip } from '@mms/mms-ui-library/components/Tooltip';
import { Skeleton } from '@mui/material';
import { useMemo } from 'react';
import { RenderCellProps } from 'react-data-grid';

import { CellWrapper } from '../../styles';
import { GridCellWithTrackedTimeProps } from '../../types';

import { TOOLTIP_FALLBACK_PLACEMENTS } from '@/components/Grid/constants';
import { TimePrefix } from '@/constants/time';
import { zIndexes } from '@/constants/zIndexes';
import { generateTimeValue } from '@/helpers/generateTimeValue';
import { minsToHrsAndMinsObject } from '@/helpers/translateTimeUtils';

const { Hours, Minutes } = TimePrefix;

export function createTrackedCellRenderer<
	T extends GridCellWithTrackedTimeProps
>(getTrackedTime: (row: T) => number | null, isShowOnlyHours: boolean = false) {
	return function (props: RenderCellProps<T, unknown>) {
		const { row } = props;

		const { showSkeleton, isActive } = row;

		const trackedTimeInMinutes = getTrackedTime(row);

		if (trackedTimeInMinutes === null) {
			return <CellWrapper disabled={!(isActive ?? true)}>-</CellWrapper>;
		}

		const { hours, minutes } = minsToHrsAndMinsObject(trackedTimeInMinutes);

		const trackedValue = useMemo(() => {
			const hourValue = generateTimeValue(hours, Hours);
			const minutesValue = generateTimeValue(minutes, Minutes);

			if (hours <= 0) {
				return minutesValue;
			}

			return isShowOnlyHours ? hourValue : `${hourValue} ${minutesValue}`;
		}, [isShowOnlyHours, hours, minutes]);

		return showSkeleton ? (
			<Skeleton animation="wave" width="100%" />
		) : (
			<Tooltip
				zIndex={zIndexes.gridTooltip}
				text={trackedValue}
				position="bottom"
				fallbackPlacements={TOOLTIP_FALLBACK_PLACEMENTS}
				useIfTextCut
			>
				<CellWrapper disabled={!(isActive ?? true)}>{trackedValue}</CellWrapper>
			</Tooltip>
		);
	};
}
