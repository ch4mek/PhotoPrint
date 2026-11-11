import styled from '@emotion/styled';
import { ActionButton } from '@mms/mms-ui-library/components/ActionButton';
import { Typography } from '@mms/mms-ui-library/components/Typography';

import { BorderRadiuses } from '@/constants/borderRadiuses';

const SQUARE_ATTACHED_FILE_WRAPPER_SIZE = '60px';

export const SquareAttachedFileWrapper = styled('div')<{
	isImage?: boolean;
}>(({ theme, isImage = false }) => ({
	position: 'relative',
	display: 'flex',
	flexDirection: 'column',
	padding: isImage ? `0px` : `${theme.spaces.xs}px`,
	borderRadius: BorderRadiuses.xs2,
	border: `1px solid ${theme.palette['graphics-divider-2']}`,
	maxWidth: SQUARE_ATTACHED_FILE_WRAPPER_SIZE,
	maxHeight: SQUARE_ATTACHED_FILE_WRAPPER_SIZE,
	overflow: 'hidden',
}));

export const FileImage = styled('img')({
	width: SQUARE_ATTACHED_FILE_WRAPPER_SIZE,
	height: SQUARE_ATTACHED_FILE_WRAPPER_SIZE,
	objectFit: 'cover',
});

export const RemoveAttachedFileButton = styled(ActionButton)(({ theme }) => ({
	position: 'absolute',
	top: `${theme.spaces.xs / 2}px`,
	right: `${theme.spaces.xs / 2}px`,
	background: theme.palette['background-white'],
	borderRadius: BorderRadiuses.s,
	width: 'auto',
	height: 'auto',
}));

export const AttachedFileText = styled(Typography)({
	maxWidth: '100%',
	overflow: 'hidden',
	whiteSpace: 'nowrap',
	textOverflow: 'ellipsis',
});
