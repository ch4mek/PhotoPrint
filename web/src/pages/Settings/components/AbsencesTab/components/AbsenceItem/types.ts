import { EntityName } from '../AbsencesList/types';

import { Absence, AgreementItem } from '@/types/absences';
import { SimpleConfigurableConfigurationItem } from '@/types/configuration';

export interface AbsenceItemProps {
	item: Absence;
	handleUpdate: (id: number | string) => void;
	handleDelete: (
		item: SimpleConfigurableConfigurationItem<number | string>,
		hasAgreements?: boolean
	) => void;
	handleAgreementDelete: (
		item: SimpleConfigurableConfigurationItem<number | string>
	) => void;
	handleAddAgreement: (
		itemId?: { itemId: string | number; item?: AgreementItem } | undefined
	) => void;
	handleAssignsOpen: (
		entityTitle: EntityName
	) => (
		itemId?: SimpleConfigurableConfigurationItem<number> | undefined
	) => void;
}
