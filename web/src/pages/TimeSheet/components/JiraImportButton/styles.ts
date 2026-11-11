import { css } from '@emotion/react';
import styled from '@emotion/styled';
import { Button } from '@mms/mms-ui-library/components/Button';

import { TimeSheetBreakpoints } from '../../constants';

const { DESKTOP_MD } = TimeSheetBreakpoints;

const ICON_BUTTON_SIZE = 36;
const JIRA_BUTTON_TOP_OFFSET = 12;
const JIRA_BUTTON_RIGHT_OFFSET = 44;

const jiraButtonBottomPosition = {
	top: 'unset',
	bottom: 0,
} as const;

const jiraButtonTopPosition = {
	top: JIRA_BUTTON_TOP_OFFSET,
	bottom: 'unset',
} as const;

export const JiraImportButton = styled(Button)<{
	isIconView?: boolean;
	isEmployeeOrSysAdminRole?: boolean;
	isPersonalModeOfPMO?: boolean;
}>(
	({ theme, isIconView, isEmployeeOrSysAdminRole, isPersonalModeOfPMO }) => css`
		${!isEmployeeOrSysAdminRole &&
		!isPersonalModeOfPMO &&
		css`
			position: absolute;
			right: ${JIRA_BUTTON_RIGHT_OFFSET}px;
			${css(jiraButtonTopPosition)};

			${theme.breakpoints.up(DESKTOP_MD)} {
				right: 0;
			}

			${theme.breakpoints.up(theme.breakpoints.desktopL)} {
				${css(jiraButtonBottomPosition)};
			}
		`}

		${isIconView &&
		css`
			min-width: unset;
			width: ${ICON_BUTTON_SIZE}px;
			padding: ${theme.spaces.xs}px;
			justify-content: end;

			&::before {
				left: 0;
			}
		`}
	`
);
