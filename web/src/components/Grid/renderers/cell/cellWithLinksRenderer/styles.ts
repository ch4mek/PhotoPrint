import styled from '@emotion/styled';

import { CellWrapper } from '@/components/Grid';

export const LinksCellWrapper = styled(CellWrapper)(({ theme }) => ({
	display: 'flex',
	flexDirection: 'column',
	gap: theme.spaces.xs,
}));

export const LinkWrapper = styled('a')(({ theme }) => ({
	color: theme.palette['text-black'],
	width: 'fit-content',
}));
