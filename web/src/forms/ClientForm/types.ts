import { SimpleConfigurableConfigurationItem } from '@/types/configuration';

export interface ClientFormValues {
	name: string;
	legalEntityName: string;
	comments: string;
	registeredLocation: string;
	saleManager: SimpleConfigurableConfigurationItem<number> | null;
	color: string;
}

export interface ClientFormSubmitValues {
	name: string;
	legalEntityName: string;
	comments: string;
	registeredLocation: number;
	salesManagerId: number | null;
	color: string;
}

export interface ClientFormProps {
	initialValues: ClientFormValues;
	handleClose: () => void;
	onSubmit: (params: ClientFormSubmitValues) => Promise<void> | void;
	id: number | undefined;
	refetch: () => void;
}
