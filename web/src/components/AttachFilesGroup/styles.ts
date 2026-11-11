import styled from '@emotion/styled';

const MIN_FILE_ITEM_WIDTH = 308;

export const AttachFilesGroupWrapper = styled('div')(({ theme }) => ({
	display: 'flex',
	flexDirection: 'column',
	gap: theme.spaces.s,
}));

export const AttachFilesWrapper = styled('div')(({ theme }) => ({
	display: 'grid',
	gap: theme.spaces.s,
	gridTemplateColumns: `repeat(auto-fit, minmax(${MIN_FILE_ITEM_WIDTH}px, 1fr))`,
}));
