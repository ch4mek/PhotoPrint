import { ImportIcon } from '@mms/mms-ui-library/icons/base';
import { useMemo } from 'react';

import { TimeSheetBreakpoints } from '../../constants';
import { useIsPersonalModeOfPMO } from '../../hooks/useIsPersonalModeOfPMO';
import { isEmployeeOrSysAdmin } from '../../utils/isEmployeeOrSysAdmin';

import { useAuth } from '@/context/AuthContext';
import { JiraImportForm } from '@/forms/JiraImportForm';
import { checkIsHasClaims } from '@/helpers/checkIsHasClaims';
import { useMobileView } from '@/hooks/useMobileView';
import { useShowModal } from '@/pages/hooks/useShowModal';
import { UserClaims } from '@/types/claims';

import { JIRA_IMPORT_BUTTON_TEXT } from './constants';
import { JiraImportButton as Button } from './styles';

export const JiraImportButton = () => {
	const { claims, role, departmentId } = useAuth();
	const isButtonDisplayed = checkIsHasClaims(
		[UserClaims.ViewJiraImport],
		claims
	);
	const isEmployeeOrSysAdminRole = useMemo(
		() => isEmployeeOrSysAdmin(role, departmentId),
		[role, departmentId]
	);
	const isPersonalModeOfPMO = useIsPersonalModeOfPMO();
	const isIconView =
		useMobileView(TimeSheetBreakpoints.MOBILE_LG) && isEmployeeOrSysAdminRole;

	const {
		isOpen: isJiraImportModalOpen,
		handleOpen: handleOpenJiraImportModal,
		handleClose: handleCloseJiraImportModal,
	} = useShowModal();

	const handleOpenJiraImport = () => {
		handleOpenJiraImportModal();
	};

	if (!isButtonDisplayed) {
		return null;
	}

	return (
		<>
			<Button
				color="secondary"
				addon={<ImportIcon />}
				isFullWidth={false}
				onClick={handleOpenJiraImport}
				{...{ isIconView, isEmployeeOrSysAdminRole, isPersonalModeOfPMO }}
			>
				{!isIconView && JIRA_IMPORT_BUTTON_TEXT}
			</Button>
			{isJiraImportModalOpen && (
				<JiraImportForm handleClose={handleCloseJiraImportModal} />
			)}
		</>
	);
};
