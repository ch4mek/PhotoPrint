import { MicroFrontendConfig } from '@/types/microfrontends';

export interface MicrofrontendsFormProps {
	id?: string | number;
	data?: MicrofrontendsFormValues;
	handleClose: VoidFunction;
	handleSubmit?: (values: MicrofrontendsFormValues) => Promise<void>;
}

export interface MicrofrontendsFormValues extends MicroFrontendConfig {}

export type MicrofrontendsFormArrayKeys = keyof Pick<
	MicrofrontendsFormValues,
	'routes' | 'claims' | 'modules'
>;
