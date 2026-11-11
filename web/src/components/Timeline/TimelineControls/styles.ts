import styled from '@emotion/styled';
import { Typography } from '@mms/mms-ui-library/components/Typography';

import { Row } from '../styles';

import { DEFAULT_CONTROL_HEIGHT } from '@/constants/commonStyles';
import { WithMobileView } from '@/types/filter';

const mobileStyles = {
	display: 'flex',
	flexWrap: 'wrap',
	justifyContent: 'space-between',
} as const;

export const TimelineControlsWrapper = styled(Row)<WithMobileView>(
	({ theme, isMobileView }) => ({
		justifyContent: 'space-between',
		gap: theme.spaces.m,

		...(isMobileView ? mobileStyles : {}),
	})
);

export const MobileTimeTypography = styled(Typography)(() => ({
	height: DEFAULT_CONTROL_HEIGHT,
	lineHeight: `${DEFAULT_CONTROL_HEIGHT}px`,
}));
