import styled from '@emotion/styled';

import { CellWrapper } from '@/components/Grid';

export const AttachedDocumentCell = styled('div')<{
	canBeClicked: boolean;
}>(({ theme, canBeClicked }) => ({
	color: canBeClicked ? theme.palette['text-black'] : 'inherit',
	cursor: canBeClicked ? 'pointer' : 'default',
	pointerEvents: canBeClicked ? 'all' : 'none',
	textDecoration: 'underline',
	overflow: 'hidden',
	whiteSpace: 'nowrap',
	textOverflow: 'ellipsis',
}));

export const AttachedDocumentCellWrapper = styled(CellWrapper)(({ theme }) => ({
	display: 'flex',
	flexDirection: 'column',
	gap: theme.spaces.xs,
}));
