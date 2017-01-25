in vec2 position;
in vec2 dimension;

void main()
{
    gl_Position = vec4(position, 0.0, 1.0);
}
