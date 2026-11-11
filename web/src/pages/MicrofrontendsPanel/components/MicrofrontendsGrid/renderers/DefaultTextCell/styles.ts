import styled from '@emotion/styled';

export const DefaultTextCellText = styled('span')(() => ({
	maxWidth: '100%',
	overflow: 'hidden',
	whiteSpace: 'nowrap',
	textOverflow: 'ellipsis',
}));
