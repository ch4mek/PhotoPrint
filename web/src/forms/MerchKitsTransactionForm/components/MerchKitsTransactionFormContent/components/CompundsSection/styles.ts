import styled from '@emotion/styled';
import { Typography } from '@mms/mms-ui-library/components/Typography';

import { BorderRadiuses } from '@/constants/borderRadiuses';

export const CompoundsWrapper = styled('div')(({ theme }) => ({
	backgroundColor: theme.palette['background-base-2'],
	borderRadius: BorderRadiuses.m,
	padding: `${theme.spaces.s}px ${theme.spaces.m}px`,
}));

export const CompoundsTitle = styled('p')(({ theme }) => ({
	...theme.typography.body['s-700'],
	marginBottom: theme.spaces.s,
	marginTop: 0,
}));

export const CompoundsList = styled('div')(({ theme }) => ({
	display: 'flex',
	flexDirection: 'column',
	gap: theme.spaces.s,
}));

export const CompoundItem = styled('div')(({ theme }) => ({
	display: 'flex',
	alignItems: 'center',
	justifyContent: 'space-between',
	gap: theme.spaces.m,
}));

export const ItemNameTypography = styled(Typography)({
	whiteSpace: 'nowrap',
	overflow: 'hidden',
	textOverflow: 'ellipsis',
	width: '100%',
	flex: 1,
});
