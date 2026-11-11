import styled from '@emotion/styled';
import { ActionButton } from '@mms/mms-ui-library/components/ActionButton';

import { ICON_SIZE, MOBILE_L_BREAKPOINT } from '../../constants';

export const DynamicFieldsGroupWrapper = styled('div')<{ gridArea?: string }>(
	({ theme, gridArea }) => ({
		display: 'flex',
		flexDirection: 'column',
		alignItems: 'flex-start',
		gap: theme.spaces.s,

		...(gridArea && { gridArea }),
	})
);

export const DynamicFieldWrapper = styled('div')(({ theme }) => ({
	display: 'flex',
	alignItems: 'center',
	gap: theme.spaces.s,

	[theme.breakpoints.down(MOBILE_L_BREAKPOINT)]: {
		display: 'grid',
		gridTemplateColumns: `1fr ${ICON_SIZE}px`,
		alignItems: 'flex-start',
	},
}));

export const FieldArrayWrapper = styled('div')(({ theme }) => ({
	display: 'flex',
	flexDirection: 'column',
	gap: theme.spaces.l,

	width: '100%',
}));

export const ActionButtonWrapper = styled(ActionButton)({
	width: `${ICON_SIZE}px`,
});
