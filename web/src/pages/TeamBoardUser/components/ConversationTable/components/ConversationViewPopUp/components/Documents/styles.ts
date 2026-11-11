import styled from '@emotion/styled';
import { Typography } from '@mms/mms-ui-library/components/Typography';

export const Value = styled(Typography)(({ theme }) => ({
	display: 'block',
	marginTop: theme.spaces.s,
}));

export const List = styled('div')(({ theme }) => ({
	display: 'grid',
	gridTemplateColumns: 'repeat(auto-fill, minmax(250px, 1fr))',
	gap: theme.spaces.s,
	marginTop: theme.spaces.s,
}));
