interface GetActivationMessageProps {
	isActive: boolean;
	entityName?: string;
}

export const getActivationMessage = ({
	isActive,
	entityName,
}: GetActivationMessageProps) =>
	` ${entityName ?? ''} will be ${isActive ? 'deactivated' : 'activated'}.`;
