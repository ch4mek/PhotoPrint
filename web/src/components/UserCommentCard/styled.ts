import styled from '@emotion/styled';

import { InformationTypography } from '../UserCard/styles';

export const TimeInformationTypography = styled(InformationTypography)(
	({ theme }) => ({
		display: 'flex',
		alignItems: 'center',
		gap: theme.spaces.xs,
	})
);
