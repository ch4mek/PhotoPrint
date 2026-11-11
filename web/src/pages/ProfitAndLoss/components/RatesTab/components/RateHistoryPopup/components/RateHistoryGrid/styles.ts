import styled from '@emotion/styled';
import { DATA_GRID_WRAPPER_CLASS_NAME } from '@mms/mms-ui-library/constants/grid';

import { InfoDataTableWrapper } from '@/components/InfoDataContainer/InfoDataTableWrapper';
import { BorderRadiuses } from '@/constants/borderRadiuses';

import { RATE_HISTORY_GRID_HEIGHT } from './constants';

const gridBottomBorderRadius = `${BorderRadiuses.m}px`;

export const RateHistoryTableWrapper = styled(InfoDataTableWrapper)({
	'& > div': {
		height: RATE_HISTORY_GRID_HEIGHT,
		overflow: 'hidden',
	},

	[`& .${DATA_GRID_WRAPPER_CLASS_NAME}`]: {
		height: '100%',

		"div[role='grid']": {
			borderRadius: `0 0 ${gridBottomBorderRadius} ${gridBottomBorderRadius}`,
		},
	},
});
