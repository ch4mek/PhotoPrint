import { ControlButtonWrapper } from './styles';
import { ControlButtonProps } from './types';

export function ControlButton({ icon, ...props }: ControlButtonProps) {
	return <ControlButtonWrapper {...props}>{icon}</ControlButtonWrapper>;
}
