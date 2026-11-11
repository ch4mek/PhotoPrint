import { PopUp } from '@mms/mms-ui-library/components/PopUp';

import { usePopupStylesConfig } from '../../hooks/usePopupStylesConfig';

import { getAddingPopUpTitle } from '@/constants/activityStatus';

import { ContactsSection } from './components/ContactsSection';
import { ContractDetailsSection } from './components/ContractDetailsSection';
import { LocationSection } from './components/LocationSection';
import { PersonalDetailsSection } from './components/PersonalDetailsSection';
import { WorkDetailsSection } from './components/WorkDetailsSection';
import { ENTITY_NAME } from './constants';
import { FormColumn, FormWrapper } from './styles';
import { UserAddingFormContentProps } from './types';

export function UserAddingFormContent({
	isLoading,
	hasHRModuleClaim,
	onClose,
	onSubmit,
	handleOutsideClose,
}: UserAddingFormContentProps) {
	const { popUpStylesConfig } = usePopupStylesConfig();

	return (
		<PopUp
			type="form"
			stylesConfig={popUpStylesConfig}
			showLoader={isLoading}
			title={getAddingPopUpTitle(ENTITY_NAME)}
			onClose={onClose}
			onShadowClick={handleOutsideClose}
			controls={{
				positiveControl: { onClick: onSubmit },
				negativeControl: { onClick: onClose },
			}}
		>
			<FormWrapper>
				<FormColumn>
					<PersonalDetailsSection />

					<ContactsSection />
				</FormColumn>

				<FormColumn>
					<WorkDetailsSection {...{ hasHRModuleClaim }} />

					<ContractDetailsSection />

					<LocationSection />
				</FormColumn>
			</FormWrapper>
		</PopUp>
	);
}
