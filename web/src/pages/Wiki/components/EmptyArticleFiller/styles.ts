import styled from '@emotion/styled';
import { EditIcon } from '@mms/mms-ui-library/icons/base/EditIcon';

import { EDIT_ICON_SIZE } from './constants';

export const EmptyArticleFillerWrapper = styled('div')(({ theme }) => ({
	height: '100%',
	display: 'flex',
	gap: theme.spaces.xs,
	alignItems: 'center',
	justifyContent: 'center',
	flexDirection: 'column' as const,
	color: theme.palette['text-black'],
}));

export const PencilIcon = styled(EditIcon)(() => ({
	height: `${EDIT_ICON_SIZE}px`,
	width: `${EDIT_ICON_SIZE}px`,
}));
