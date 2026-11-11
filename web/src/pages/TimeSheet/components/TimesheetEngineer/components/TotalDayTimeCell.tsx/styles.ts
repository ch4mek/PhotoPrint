import styled from '@emotion/styled';
import { Typography } from '@mms/mms-ui-library/components/Typography';

import { NORMAL_WORK_DAY_IN_MINUTES } from '../../constants';

import { ColumnTotalTypographyProps } from './types';

export const ColumnTotalTypography = styled(
	Typography
)<ColumnTotalTypographyProps>(({ theme, trackedInMinutes }) => {
	let color: string = theme.palette['base-4'];

	if (trackedInMinutes) {
		color = theme.palette['feedback-danger'];

		if (trackedInMinutes === NORMAL_WORK_DAY_IN_MINUTES) {
			color = theme.palette['feedback-success'];
		}

		if (trackedInMinutes > NORMAL_WORK_DAY_IN_MINUTES) {
			color = theme.palette.trackedTimeCell!.info || color;
		}
	}

	return {
		color,
	};
});

export const DayTotalTimeColumnWrapper = styled('div')`
	display: flex;
	flex-direction: column;
	align-items: center;
`;
