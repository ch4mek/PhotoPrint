import styled from '@emotion/styled';
import { ActionButton } from '@mms/mms-ui-library/components/ActionButton';

import { BorderRadiuses } from '@/constants/borderRadiuses';
import { IconSize } from '@/constants/iconSizes';
import { ASSIGNMENT_FORM_MOBILE_BREAKPOINT } from '@/forms/Absence/AbsenceAssignmentForm/constants';

export const ControlButtonWrapper = styled(ActionButton)<{ gridArea: string }>(
	({ theme, gridArea }) => ({
		width: IconSize.XL,
		height: IconSize.XL,

		borderRadius: BorderRadiuses.m,
		border: `1px solid ${theme.palette['graphics-divider-1']}`,
		gridArea,

		'&: hover': {
			backgroundColor: theme.palette['hover-base-2'],
			color: theme.palette['graphics-primary-2'],
		},

		[theme.breakpoints.down(ASSIGNMENT_FORM_MOBILE_BREAKPOINT)]: {
			transform: 'rotate(90deg)',
		},
	})
);
