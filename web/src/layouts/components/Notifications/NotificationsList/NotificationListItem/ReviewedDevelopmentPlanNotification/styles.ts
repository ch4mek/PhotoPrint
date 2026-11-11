import styled from '@emotion/styled';

import { NavigationNotificationWrapper } from '../../styles';

export const DevelopmentPlanReviewedNotificationWrapper = styled(
	NavigationNotificationWrapper
)(({ theme }) => ({
	display: 'flex',
	gap: theme.spaces.xs,
	alignItems: 'center',
}));
