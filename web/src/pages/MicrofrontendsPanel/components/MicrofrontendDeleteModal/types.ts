export interface MicrofrontendDeleteModalProps {
	name: string;
	handleClose: VoidFunction;
	handleDelete: (id: string | number) => Promise<void>;
	id?: string | number;
}
