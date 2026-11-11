import { AgreementItem } from '@/types/absences';
import { SimpleConfigurableConfigurationItem } from '@/types/configuration';

export type AgreementsListProps = {
	agreements: AgreementItem[];
	absenceId: string | number;
	handleAddAgreement: (
		itemId?: { itemId: string | number; item?: AgreementItem } | undefined
	) => void;
	handleAgreementDelete: (
		item: SimpleConfigurableConfigurationItem<number | string>
	) => void;
	handleAgreementAssigns: (
		item: SimpleConfigurableConfigurationItem<number>
	) => void;
};
