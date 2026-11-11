import { Tabs } from '@mms/mms-ui-library/components/Tabs';
import { useHasScreenWidth } from '@mms/mms-ui-library/hooks/useHasScreenWidth';
import { breakpoints } from '@mms/mms-ui-library/theme';
import { useMemo } from 'react';
import { useTranslation } from 'react-i18next';

import { userProfileTabs } from '../../constansts';
import { UserProfileTabsId } from '../../types';

import { minHeightTabRestriction } from '@/components/UserProfile/components/UserProfileTabs/constants';
import { departmentIdsWithCertificates } from '@/constants/departments';
import { isRegularEmployee } from '@/constants/roles';
import { useAuth } from '@/context/AuthContext';
import { checkIsHasClaims } from '@/helpers/checkIsHasClaims';
import { useResize } from '@/hooks/useResize';

import { TabsContainer, TabsContentWrapper } from './styles';
import { UserProfileTabsProps } from './types';

const { tablet } = breakpoints;

export function UserProfileTabs({
	activeTab,
	onChange,
	children,
}: UserProfileTabsProps) {
	const [, windowHeight] = useResize();
	const { position, departmentId, claims } = useAuth();
	const isMobileView = !useHasScreenWidth(tablet);

	const { t } = useTranslation('profile', { keyPrefix: 'tabs' });

	const minHeight = useMemo(() => {
		const isEducationAllowed = Boolean(
			departmentId &&
				position &&
				departmentIdsWithCertificates.includes(departmentId) &&
				isRegularEmployee(position)
		);

		const restrictions: Partial<Record<UserProfileTabsId, boolean>> = {
			[UserProfileTabsId.EDUCATION]: isEducationAllowed,
			[UserProfileTabsId.ABSENCES]: false,
			[UserProfileTabsId.SALARY]: true,
			[UserProfileTabsId.JIRA_SETTINGS]: true,
		};

		if (restrictions[activeTab as UserProfileTabsId]) {
			return (
				minHeightTabRestriction(isMobileView)[activeTab as UserProfileTabsId] ??
				null
			);
		}

		return null;
	}, [activeTab, departmentId, position, isMobileView]);

	const tabsToShow = useMemo(
		() =>
			userProfileTabs.flatMap((tab) =>
				!tab.requiredClaims || checkIsHasClaims(tab.requiredClaims, claims)
					? [{ ...tab, text: t(tab.text) }]
					: []
			),
		[claims, t]
	);

	return (
		<TabsContainer
			windowHeight={windowHeight}
			isHeightUnrestricted={
				activeTab === UserProfileTabsId.ABSENCES ||
				activeTab === UserProfileTabsId.SUMMARY
			}
			minHeight={minHeight}
		>
			<Tabs activeTab={activeTab} onChangeTab={onChange} data={tabsToShow} />
			<TabsContentWrapper
				scroll="horizontal"
				maxWidth="100%"
				maxHeight="100%"
				hasReducedTopPadding={activeTab === UserProfileTabsId.JIRA_SETTINGS}
			>
				{children}
			</TabsContentWrapper>
		</TabsContainer>
	);
}
