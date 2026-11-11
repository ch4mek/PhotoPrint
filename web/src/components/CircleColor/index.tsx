import { CircleColorElement, CircleColorWrapper } from './styles';

export function CircleColor({ color }: { color: string }) {
	return (
		<CircleColorWrapper>
			<CircleColorElement color={color} />
		</CircleColorWrapper>
	);
}
