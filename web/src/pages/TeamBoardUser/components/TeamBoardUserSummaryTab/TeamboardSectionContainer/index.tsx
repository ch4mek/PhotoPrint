import { useContext } from 'react';

import { tabs } from '../../../constants';
import { PersonalDataContext } from '../../../context/personalDataContext';
import { TabContext } from '../../../context/tabContext';
import { hasAccess } from '../../../utils';

import { InfoDataContainer } from '@/components/InfoDataContainer';
import { InfoDataContainerProps } from '@/components/InfoDataContainer/types';
import { useAuth } from '@/context/AuthContext';
import { checkIsHasClaims } from '@/helpers/checkIsHasClaims';

export function TeamboardSectionContainer({
	tabId,
	handleAddClick,
	...props
}: InfoDataContainerProps) {
	const { handleTabChange } = useContext(TabContext);
	const { accessLevel } = useContext(PersonalDataContext);
	const { claims } = useAuth();

	const tabConfig = tabs.find((t) => t.id === tabId);

	const hasRequiredClaims =
		!tabConfig?.requiredClaims ||
		checkIsHasClaims(tabConfig.requiredClaims, claims);

	const canInteract =
		hasAccess(accessLevel, tabConfig?.accessLevels ?? []) && hasRequiredClaims;

	const handleClickView = () => {
		if (tabId) {
			handleTabChange(tabId);
		}
	};

	const resolvedHandleAddClick =
		!tabId || canInteract ? handleAddClick : undefined;

	return (
		<InfoDataContainer
			{...props}
			tabId={tabId}
			handleAddClick={resolvedHandleAddClick}
			handleClickView={canInteract ? handleClickView : undefined}
		/>
	);
}
