import styled from '@emotion/styled';
import { Typography } from '@mms/mms-ui-library/components/Typography';

export const GridMenuWrapper = styled.div(({ theme }) => ({
	display: 'flex',
	flexDirection: 'column',
	gap: theme.spaces.s,
}));

export const GridContainer = styled.div(({ theme }) => ({
	display: 'grid',
	gridTemplateColumns: 'repeat(4, 1fr)',
	gap: theme.spaces.s,
}));

export const MenuHeader = styled(Typography)(({ theme }) => ({
	color: theme.palette['text-gray'],
	...theme.typography.headlines.small,
}));
