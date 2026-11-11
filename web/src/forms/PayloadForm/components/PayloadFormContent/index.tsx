import { useTheme } from '@emotion/react';
import { PopUp } from '@mms/mms-ui-library/components/PopUp';
import { useMemo } from 'react';

import { PayloadFormTabs, POPUP_TITLE } from '../../constants';
import { PaylaodDetails } from '../PayloadDetails';

import { CommonProjectAccessTab } from '@/components/CommonProjectAccessTab';
import { TabPanel } from '@/components/TabPanel';
import { getPopUpStylesConfig } from '@/forms/PayloadForm/utils/getPopUpStylesConfig';
import { useResize } from '@/hooks/useResize';

import {
	PAYLOAD_TABS,
	PROJECT_ACCESS_TAB_HEIGHT,
	PROJECT_DETAILS_TAB_HEIGHT,
} from './constants';
import { TabWrapper } from './styles';
import { PayloadFormContentProps } from './types';

export function PayloadFormContent({
	activeTab,
	isLoaderVisible,
	user,
	onActiveTabChange,
	handleClose,
	handleOutsideClose,
	submitForm,
}: PayloadFormContentProps) {
	const theme = useTheme();
	const [, windowHeight] = useResize();

	const popUpStylesConfig = useMemo(
		() => getPopUpStylesConfig(theme, activeTab, windowHeight),
		[theme, activeTab, windowHeight]
	);

	return (
		<PopUp
			onClose={handleClose}
			onShadowClick={handleOutsideClose}
			title={POPUP_TITLE}
			type="form"
			tabs={{
				fullWidth: false,
				data: PAYLOAD_TABS,
				activeTab,
				onChangeTab: onActiveTabChange,
			}}
			stylesConfig={popUpStylesConfig}
			controls={
				activeTab === PayloadFormTabs.Details
					? {
							positiveControl: {
								onClick: submitForm,
								disabled: isLoaderVisible,
							},
							negativeControl: { onClick: handleClose },
					  }
					: undefined
			}
			showLoader={isLoaderVisible}
		>
			<TabPanel value={activeTab} index={PayloadFormTabs.Details}>
				<TabWrapper height={PROJECT_DETAILS_TAB_HEIGHT}>
					<PaylaodDetails />
				</TabWrapper>
			</TabPanel>
			<TabPanel value={activeTab} index={PayloadFormTabs['Projects access']}>
				<TabWrapper height={PROJECT_ACCESS_TAB_HEIGHT}>
					<CommonProjectAccessTab {...{ user }} isUserTab />
				</TabWrapper>
			</TabPanel>
		</PopUp>
	);
}
