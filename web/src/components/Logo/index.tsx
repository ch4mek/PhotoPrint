import { Title } from './styles';
import { LogoProps } from './types';

export function Logo({ title, ref }: LogoProps) {
	return <Title ref={ref}>{title}</Title>;
}
